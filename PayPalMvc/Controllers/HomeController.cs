﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayPalMvc.Models;
using System.Configuration;
using System.Net;
using System.Text;
using System.IO;

namespace PayPalMvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult PostToPayPal(string item, string amount)
        {

            PayPal paypal = new PayPal();
            paypal.cmd = "_xclick";
            paypal.business = ConfigurationManager.AppSettings["BusinessAccountKey"];

            bool useSandbox = Convert.ToBoolean(ConfigurationManager.AppSettings["UseSandbox"]);

            if (useSandbox)
                ViewBag.actionUrl = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            else
                ViewBag.actionUrl = "https://www.paypal.com/cgi-bin/webscr";

            paypal.cancel_return = ConfigurationManager.AppSettings["CancelUrl"];
            paypal.@return = ConfigurationManager.AppSettings["ReturnUrl"] + "?item_number=12345";
            paypal.notify_url = ConfigurationManager.AppSettings["NotifyUrl"];
            paypal.currency_code = ConfigurationManager.AppSettings["CurrencyCode"];

            paypal.item_name = item;
            paypal.amount = amount;

            return View(paypal);
        }


        public ActionResult RedirectFromPaypal(string tx)
        {

            var resp = GetPayPalResponse(tx, true);
            ViewBag.tx = resp;

            return View();
        }


        string GetPayPalResponse(string tx, bool useSandbox)
        {

            string authToken = "7tBnRytY2SmPuvY6LCoqZ4uqAivCP4zSr_kibkj34-Egd71IZUfvPwWHJB4";
            string txToken = tx;
            string query = "cmd=_notify-synch&tx=" + txToken + "&at=" + authToken;

            //Post back to either sandbox or live
            string strSandbox = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            //string strLive = "https://www.paypal.com/cgi-bin/webscr";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strSandbox);

            //Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = query.Length;

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.DefaultConnectionLimit = 9999;


            //Send the request to PayPal and get the response
            StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
            streamOut.Write(query);
            streamOut.Close();
            StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
            string strResponse = streamIn.ReadToEnd();
            string result = string.Empty;
            streamIn.Close();

            Dictionary<string, string> results = new Dictionary<string, string>();
            if (strResponse != "")
            {
                StringReader reader = new StringReader(strResponse);
                string line = reader.ReadLine();

                if (line == "SUCCESS")
                {

                    while ((line = reader.ReadLine()) != null)
                    {
                        results.Add(line.Split('=')[0], line.Split('=')[1]);

                    }

                    result = "<p><h3>Your order has been received.</h3></p>";
                    result += "<b>Details</b><br>";
                    result += "<li>Name: " + results["first_name"] + " " + results["last_name"] + "</li>";
                    result += "<li>Item: " + results["item_name"] + "</li>";
                    result += "<li>Amount: " + results["payment_gross"] + "</li>";
                    result += "<hr>";
                }
                else if (line == "FAIL")
                {
                    // Log for manual investigation
                    Response.Write("Unable to retrive transaction detail");
                }
            }
            else
            {
                //unknown error
                Response.Write("ERROR");
            }


            return result;
        }
    }
}