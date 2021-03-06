﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestPaperCreator.MODEL.Membership;

namespace TestPaperCreator.Controllers.TestPaperService
{
    public class ViewQuestionController : Controller
    {
        // GET: ViewQuestion
        public ActionResult Index()
        {
            if (Session["user"] != null)
            {
                string sessions = Session["user"].ToString();
                User user = new User();
                user = BLL.Utility.Utility.DeSerialize<User>(user, sessions);
                bool result = BLL.Membership.Membership.Login(user);
                if (result == false)
                {
                    ViewBag.IsLogin = true;
                    Response.Redirect("/Membership/Login");
                }
                else
                {
                    ViewBag.IsLogin = true;
                    ViewBag.Username = user.username;
                }
            }
            else
            {
                if (Request.Cookies["UserInfo"] != null)
                {
                    User user = new User();
                    user = BLL.Utility.Utility.DeSerialize<User>(user, Request.Cookies["UserInfo"].Value.ToString());
                    string username = user.username;
                    string password = user.password;
                    ViewBag.UserName = username;
                    bool result = BLL.Membership.Membership.Login(user);
                    if (result == false)
                    {
                        ViewBag.IsLogin = false;
                        Response.Redirect("/Membership/login");
                    }
                    else
                    {
                        ViewBag.IsLogin = true;
                        ViewBag.Username = username;
                    }
                }
                else
                {
                    Response.Redirect("/Membership/login");
                }
            }
            List<MODEL.TestPaper.Question> questionlist = BLL.TestPaperService.TestPaperService.GetQuestionList(0, 0, 0, 0, null);
            ViewBag.questionlist = questionlist;
            return View();
        }
    }
}