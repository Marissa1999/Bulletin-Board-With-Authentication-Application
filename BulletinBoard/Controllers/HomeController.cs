using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TotpAuth;

namespace BulletinBoard.Controllers
{
    public class HomeController : Controller
    {
        Models.MessagesEntities db = new Models.MessagesEntities();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            //find the user record
            string username = collection["username"];
            Models.User theUser = db.Users.SingleOrDefault(u => u.username.Equals(username));
            if(theUser != null &&
                Crypto.VerifyHashedPassword(theUser.password_hash, collection["password_hash"]))
            {
                if (theUser.secret != null)
                {
                    Totp totp = new Totp(theUser.secret);
                    string theCode = totp.AuthenticationCode;
                    if (theCode.Equals(collection["validation"]))
                    {
                        Session["user_id"] = theUser.user_id;
                        return RedirectToAction("Index", "Post");
                    }
                    else
                    {
                        ViewBag.error = "Wrong Username/Password/2FA combination!";
                        return View();
                    }
                }
                else
                {
                    Session["user_id"] = theUser.user_id;
                    return RedirectToAction("Index", "Post");
                }
            }
            else
            {
                ViewBag.error = "Wrong Username/Password/2FA combination!";
                return View();
            } 
                
        }


        // GET: Home/Details/5
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }

        private static Random random = new Random();
        private static string RandomBase32String(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        // GET: Home/Create
        public ActionResult Register()
        {
            //generate a string like this
            //otpauth://totp/Example:alice@google.com?secret=JBSWY3DPEHPK3PXP&issuer=Example

            //secret that has 16 chars A-Z, not 0, not 1, but 2-7
            string secret = RandomBase32String(16);
            string otpauth = "otpauth://totp/CoolApplication:someaccount?secret="+
                secret+ "&issuer=CoolApplication";
            
            //to generate a QR code
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(otpauth, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            ImageConverter converter = new ImageConverter();
            ViewBag.QRCode = (byte[])converter.ConvertTo(qrCodeImage, typeof(byte[]));
            Session["secret"] = secret;


            return View();
        }

        // POST: Home/Create
        [HttpPost]
        public ActionResult Register(FormCollection collection)
        {
            try
            {
                string secret = Session["secret"].ToString();
                Totp totp = new Totp(secret);

                String theSecret = null;
                if (totp.AuthenticationCode.Equals(collection["validation"].Trim()))
                {
                    //use the 2fa authentication as a login feature
                    //add this secret to the user information
                    theSecret = secret;
                }

                // TODO: Add insert logic here
                string username = collection["username"];
                Models.User theUser = db.Users.SingleOrDefault(u => u.username.Equals(username));
                if (theUser != null)
                    return RedirectToAction("Register");//todo:provide feedback

                Models.User newUser = new Models.User()
                {
                    username = collection["username"],
                    password_hash = Crypto.HashPassword(collection["password_hash"]),
                    secret = theSecret
                };
                db.Users.Add(newUser);
                db.SaveChanges();
                
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

      
    }
}
