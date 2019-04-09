﻿using DataAccessLayer.Database;
using ManagerLayer.Login;
using DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApi_PointMap.Models;
using System.Web.Http.Controllers;
using static ServiceLayer.Services.ExceptionService;
using ManagerLayer.UserManagement;

namespace WebApi_PointMap.Controllers
{
    public class UserController : ApiController
    {
        UserLoginManager _userLoginManager;
        UserManagementManager _userManagementManager;

        // POST api/user/login
        [HttpPost]
        [Route("api/user/login")]
        public IHttpActionResult LoginFromSSO([FromBody] LoginDTO requestPayload)
        {
            if (!ModelState.IsValid || requestPayload == null)
            {
                return Content((HttpStatusCode)412, ModelState);
            }
            _userLoginManager = new UserLoginManager();
            Guid userSSOID;
            try
            {
                // check if valid SSO ID format
                userSSOID = Guid.Parse(requestPayload.SSOUserId);
            }
            catch (Exception)
            {
                return Content((HttpStatusCode)400, "Invalid SSO ID");
            }
            using (var _db = new DatabaseContext())
            {
                LoginManagerResponseDTO loginAttempt;
                try
                {
                    loginAttempt = _userLoginManager.LoginFromSSO(
                        _db,
                        requestPayload.Email,
                        userSSOID,
                        requestPayload.Signature,
                        requestPayload.PreSignatureString());
                }
                catch (InvalidTokenSignatureException ex)
                {
                    return Content((HttpStatusCode)401, ex.Message);
                }
                catch (InvalidDbOperationException ex)
                {
                    return Content((HttpStatusCode)500, ex.Message);
                }
                catch (InvalidEmailException ex)
                {
                    return Content((HttpStatusCode)400, ex.Message);
                }

                LoginResponseDTO response = new LoginResponseDTO
                {
                    redirectURL = "https://pointmap.net/#/login/?token=" + loginAttempt.Token
                };

                return Ok(response);
            }
        }
    }
}
