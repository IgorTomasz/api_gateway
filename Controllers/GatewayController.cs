using api_gateway.models;
using api_gateway.models.AccountDTOs;
using api_gateway.models.DTOs;
using api_gateway.models.GameDTOs;
using api_gateway.models.PaymentDTOs;
using api_gateway.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace api_gateway.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class GatewayController : ControllerBase
	{
		private readonly IGatewayService _gatewayService;
		private readonly ILogger<GatewayController> _logger;
		public GatewayController(IGatewayService gatewayService, ILogger<GatewayController> logger)
		{
			_gatewayService = gatewayService;
			_logger = logger;
		}

		[HttpPost("user/register")]
		public async Task<IActionResult> RegisterUser(UserRegisterRequest registerRequest)
		{
			HttpResponseModel resp = await _gatewayService.RegisterUser("account/User/auth/register", registerRequest);
			if (!resp.Success)
			{
				return Ok(new HttpResponseModel
				{
					Success = false,
					Error = resp.Error
				});
			}
			Guid userId = Guid.Parse(resp.Message.ToString());
			await _gatewayService.CreateAccount("payment/Payment/account/create", new CreateAccountRequestMicroservice { UserId = userId });
			return Created("", new HttpResponseModel
			{
				Success = true,
				Message = userId
			});
		}

		[HttpPost("user/login")]
		public async Task<IActionResult> LoginUser(UserLoginRequest loginRequest)
		{
			var isLogged = await _gatewayService.LoginUser("account/User/auth/login", loginRequest);

			if (!isLogged.Success && isLogged.UserId==Guid.Empty)
			{
				return Ok(new HttpResponseModel
				{
					Success = false,
					Error = isLogged.Error
				});
			}

			UserProfileResponse responseProfile = await _gatewayService.GetUserProfile("account/User/profile",isLogged.UserId);


			UserTokensResponse tokens = _gatewayService.GenerateJwtTokens(responseProfile.User);

			UserCreateSessionRequest sessionRequest = new UserCreateSessionRequest
			{
				UserId = isLogged.UserId,
				DeviceInfo = Request.Headers["User-Agent"].ToString(),
				IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? Request.Headers["X-Forwarded-For"].FirstOrDefault(),
				RefToken = tokens.refToken				
			};

			Guid sessionId = await _gatewayService.CreateUserSession("account/UserSession/auth/session/create", sessionRequest);

			return Ok(new HttpResponseModel
			{
				Success = true,
				Message = new
				{
					SessionId = sessionId,
					Token = tokens.jwtToken,
					RefToken = tokens.refToken
				}
			});
		}

		[Authorize]
		[HttpPost("profile/update-password")]
		public async Task<IActionResult> UpdateUserPassword(ChangeUserPasswordRequest passwordRequest)
		{
			HttpResponseModel userInfo = await _gatewayService.GetUserInfo("account/UserSession/profile/userInfo", passwordRequest.SessionId);

			if (!userInfo.Success)
			{
				return Ok(new HttpResponseModel
				{
					Success = false,
					Error = "There is no user connected to that session"
				});
			}

			Guid userId = Guid.Parse(userInfo.Message.ToString());

			HttpResponseModel responseModel = await _gatewayService.ChangeUserPassword("account/User/profile/update-password", new ChangeUserPasswordUserMicroservice { userId = userId, newPassword = passwordRequest.Password });

			if (!responseModel.Success)
			{
				return Ok(new HttpResponseModel
				{
					Success = false,
					Error = "Something went wrong while changing user password"
				});
			}

			return Ok(responseModel);
		} 

		[Authorize]
		[HttpGet("profile/{sessionId}")]
		public async Task<IActionResult> GetUserProfile(Guid sessionId)
		{
			HttpResponseModel userInfo = await _gatewayService.GetUserInfo("account/UserSession/profile/userInfo", sessionId);

			if (!userInfo.Success)
			{
				return Ok(new HttpResponseModel
				{
					Success = false,
					Error = "There is no user connected to that session"
				});
			}

			Guid userId = Guid.Parse(userInfo.Message.ToString());

			UserProfileResponse user = await _gatewayService.GetUserProfile("account/User/profile", userId);

			return Ok(new HttpResponseModel
			{
				Success = true,
				Message = user.User
			});

		}

		[HttpPost("profile/refresh")]
		public async Task<IActionResult> GetNewToken(UserRefreshTokenRequest refRequest)
		{
			HttpResponseModel userInfo = await _gatewayService.GetUserInfo("account/UserSession/profile/userInfo", refRequest.SessionId);

			if (!userInfo.Success)
			{
				return Ok(new HttpResponseModel
				{
					Success = false,
					Error = "There is no user connected to that session"
				});
			}

			Guid userId = Guid.Parse(userInfo.Message.ToString());

			UserRefreshTokenRequestMicroservice request = new UserRefreshTokenRequestMicroservice
			{
				SessionId = refRequest.SessionId,
				UserId = userId
			};

			HttpResponseModel responseToken = await _gatewayService.GetUserRefToken("account/UserSession/auth/refresh-token", request);

			if (!responseToken.Success)
			{
				return Ok(new HttpResponseModel
				{
					Success = false,
					Error = responseToken.Error
				});
			}

			UserProfileResponse responseProfile = await _gatewayService.GetUserProfile("account/User/profile", userId);

			string token = responseToken.Message.ToString();

			if (token.Equals(refRequest.RefToken))
			{
				UserTokensResponse tokens = _gatewayService.GenerateJwtTokens(responseProfile.User);

				UpdateSessionWithRefTokenRequestMicroservice sessionUpdateRequest = new UpdateSessionWithRefTokenRequestMicroservice
				{
					SessionId = refRequest.SessionId,
					RefToken = tokens.refToken
				};

				HttpResponseModel model = await _gatewayService.UpdateSessionOnRefreshToken("account/UserSession/auth/session/update", sessionUpdateRequest);

				return Ok(new HttpResponseModel
				{
					Success = true,
					Message = new
					{
						SessionId = model.Message,
						Token = tokens.jwtToken,
						RefToken = tokens.refToken
					}
				});

			}

			return BadRequest(new HttpResponseModel
			{
				Success = false,
				Error = "There is no matching refresh token"
			});

		}

		[Authorize]
		[HttpPost("payments/handle-deposit")]
		public async Task<IActionResult> HandleDeposit(HandlePaymentRequest request)
		{
			HttpResponseModel userInfo = await _gatewayService.GetUserInfo("account/UserSession/profile/userInfo", request.SessionId);

			if (!userInfo.Success)
			{
				return Ok(new HttpResponseModel
				{
					Success = false,
					Error = "There is no user connected to that session"
				});
			}

			if (request.PaymentMethod == null)
			{
				return Ok(new HttpResponseModel
				{
					Success = false,
					Error = "There is no defined payment method"
				});
			}

			if (!("CardPaypalBlik").Contains(request.PaymentMethod))
			{
				return Ok(new HttpResponseModel
				{
					Success = false,
					Error = "Wrong payment method"
				});
			}

			Guid userId = Guid.Parse(userInfo.Message.ToString());

			HandlePaymentRequestMicroservice handlePaymentRequest = new HandlePaymentRequestMicroservice
			{
				UserId = userId,
				Amount = request.Amount,
				MetaData = new Dictionary<string, object>
				{
					{"PaymentMethod", request.PaymentMethod},
					{"TransactionType", "Deposit" }
				}
			};

			if (request.MetaData != null)
			{
				foreach (var item in request.MetaData)
				{
					handlePaymentRequest.MetaData.Add(item.Key, item.Value);
				}
			}

			HttpResponseModel resp = await _gatewayService.HandlePayment("payment/Payment/payments/handle-payment", handlePaymentRequest);

			if (!resp.Success)
			{
				return Ok(new HttpResponseModel
				{
					Success = false,
					Error = resp.Error
				});
			}

			return Created("", new HttpResponseModel
			{
				Success = true,
				Message = resp
			});

		}

		[Authorize]
		[HttpPost("payments/handle-withdraw")]
		public async Task<IActionResult> HandleWithdraw(HandlePaymentRequest request)
		{
			HttpResponseModel userInfo = await _gatewayService.GetUserInfo("account/UserSession/profile/userInfo", request.SessionId);

			if (!userInfo.Success)
			{
				return Ok(new HttpResponseModel
				{
					Success = false,
					Error = "There is no user connected to that session"
				});
			}

			if (request.PaymentMethod == null)
			{
				return Ok(new HttpResponseModel
				{
					Success = false,
					Error = "There is no defined payment method"
				});
			}

			if (!("CardPaypalBlik").Contains(request.PaymentMethod))
			{
				return Ok(new HttpResponseModel
				{
					Success = false,
					Error = "Wrong payment method"
				});
			}

			Guid userId = Guid.Parse(userInfo.Message.ToString());

			HandlePaymentRequestMicroservice handlePaymentRequest = new HandlePaymentRequestMicroservice
			{
				UserId = userId,
				Amount = request.Amount,
				MetaData = new Dictionary<string, object>
				{
					{"PaymentMethod", request.PaymentMethod},
					{"TransactionType", "Withdraw" }
				}
			};

			if(request.MetaData != null)
			{
				foreach (var item in request.MetaData)
				{
					handlePaymentRequest.MetaData.Add(item.Key, item.Value);
				}
			}

			HttpResponseModel resp = await _gatewayService.HandlePayment("payment/Payment/payments/handle-payment", handlePaymentRequest);

			if (!resp.Success)
			{
				return Ok(new HttpResponseModel
				{
					Success = false,
					Error = resp.Error
				});
			}

			return Created("", resp);

		}

		[Authorize]
		[HttpGet("profile/balance/{sessionId}")]
		public async Task<IActionResult> GetUserBalance(Guid sessionId)
		{
			HttpResponseModel userInfo = await _gatewayService.GetUserInfo("account/UserSession/profile/userInfo", sessionId);

			if (!userInfo.Success)
			{
				return Ok(new HttpResponseModel
				{
					Success = false,
					Error = "There is no user connected to that session"
				});
			}

			Guid userId = Guid.Parse(userInfo.Message.ToString());

			HttpResponseModel resp = await _gatewayService.GetUserBalanceOrTransaction("payment/Payment/wallet/balance", userId);

			if (!resp.Success)
			{
				return Ok(resp);
			}

			return Ok(resp);
		}

		[Authorize]
		[HttpGet("profile/transactions/{sessionId}")]
		public async Task<IActionResult> GetUserTransactions(Guid sessionId)
		{
			HttpResponseModel userInfo = await _gatewayService.GetUserInfo("account/UserSession/profile/userInfo", sessionId);

			if (!userInfo.Success)
			{
				return Ok(new HttpResponseModel
				{
					Success = false,
					Error = "There is no user connected to that session"
				});
			}

			Guid userId = Guid.Parse(userInfo.Message.ToString());

			HttpResponseModel resp = await _gatewayService.GetUserBalanceOrTransaction("payment/Payment/wallet/transactions", userId);

			if (!resp.Success)
			{
				return Ok(resp);
			}

			return Ok(resp);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("adm/users")]
		public async Task<IActionResult> GetAllUsers()
		{
			return Ok(new HttpResponseModel
			{
				Success = true,
				Message = await _gatewayService.GetAllUsers("account/User/adm/users")
			});
		}

		[Authorize(Roles = "Admin")]
		[HttpPut("adm/games/update")]
		public async Task<IActionResult> UpdateGames(AdminGameUpdate request)
		{
			return Ok(await _gatewayService.UpdateGames("adm/games/update",request));
		}

		
		[HttpGet("games")]
		public async Task<IActionResult> GetAllGames()
		{
			var games = await _gatewayService.GetAllGames("Game/games");
			
			return Ok(new HttpResponseModel
			{
				Success = true,
				Message = games.Games
			});
		}

		[HttpGet("games/{category}")]
		public async Task<IActionResult> GetAllGames(string category)
		{
			GameCategory gameCategory = Enum.Parse<GameCategory>(category);
			var games = await _gatewayService.GetAllGamesByCategory("Game/games", gameCategory);

			return Ok(new HttpResponseModel
			{
				Success = true,
				Message = games.Games
			});
		}

		[Authorize]
		[HttpPost("games/{game}")]
		public async Task<IActionResult> ProcessGameRequest(string game, ProcessGameRequest request)
		{
			GameType gameType = (GameType)Enum.Parse(typeof(GameType), game);

			HttpResponseModel userInfo = await _gatewayService.GetUserInfo("account/UserSession/profile/userInfo", request.UserSessionId);

			Guid userId = Guid.Parse(userInfo.Message.ToString());

			if (request.Action == ActionType.Start)
			{
				HttpResponseModel resp = await _gatewayService.HandlePayment("payment/Payment/payments/handle-payment", new HandlePaymentRequestMicroservice
				{
					UserId = userId,
					Amount = request.BetAmount,
					MetaData = new Dictionary<string, object>
								{
									{"PaymentMethod", "System"},
									{"TransactionType", "GameBet" }
								}
				});

				if (!resp.Success)
				{
					return Ok(new HttpResponseModel
					{
						Success = false,
						Error = resp.Error
					});
				}

				ProcessGameStartResponse resultStart = await _gatewayService.ProcessGameStart("Game/process", new ProcessGameRequestMicroservice
				{
					Type = gameType,
					UserId = userId,
					GameSessionId = null,
					UserSessionId = request.UserSessionId,
					Action = request.Action,
					BetAmount = request.BetAmount,
					Data = request.Data
				});

				if (!resultStart.Success)
				{
					return Ok(new HttpResponseModel
					{
						Success = false,
						Error = resultStart.Error
					});
				}

				return Ok(resultStart);
			}

	
			if (request.Action == ActionType.Move)
			{
							
				var ifEnded = await _gatewayService.CheckIfGameAlreadyEnded("Game/games/ended", request.GameSessionId);

				if (ifEnded.Success)
				{
					return Conflict(new HttpResponseModel
					{
						Success = false,
						Error = "Game already ended!"
					});
				}
			}

			ProcessGameResponse result = await _gatewayService.ProcessGame("Game/process", new ProcessGameRequestMicroservice
			{
				Type = gameType,
				UserId = userId,
				GameSessionId = request.GameSessionId,
				UserSessionId = request.UserSessionId,
				Action = request.Action,
				BetAmount = request.BetAmount,
				Data = request.Data
			});

			if (!result.Success)
			{
				return Conflict(new HttpResponseModel
				{
					Success = false,
					Error = result.Error
				});
			}

			if (result.Message.Status == GameStatus.EndedWin)
			{
				HttpResponseModel payWinResponse = await _gatewayService.HandlePayment("payment/Payment/payments/handle-payment", new HandlePaymentRequestMicroservice
				{
					UserId = userId,
					Amount = result.Message.Result,
					MetaData = new Dictionary<string, object>
								{
									{"PaymentMethod", "System"},
									{"TransactionType", "GameWin" }
								}
				});

				if (!payWinResponse.Success)
				{
					return Conflict(new HttpResponseModel
					{
						Success = false,
						Error = payWinResponse.Error
					});
				}
			}

			return Ok(result);
					
			}
		}
	}

