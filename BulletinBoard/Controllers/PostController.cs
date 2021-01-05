using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BulletinBoard.Controllers
{
    public class PostController : Controller
    {
        Models.MessagesEntities db = new Models.MessagesEntities();
        // GET: Home
        public ActionResult Index()
        {
            if (Session["user_id"] == null)
                return RedirectToAction("Index", "Home");
            return View(db.Posts);
        }

        // GET: Home/Create
        public ActionResult Create()
        {
            if (Session["user_id"] == null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        // POST: Home/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            if (Session["user_id"] == null)
                return RedirectToAction("Index", "Home");
            try
            {
                // TODO: Add insert logic here
                Models.Post newPost = new Models.Post()
                {
                    message = collection["message"],
                    //author = collection["author"]
                };
                db.Posts.Add(newPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Home/Edit/5
        public ActionResult Edit(int id)
        {
            if (Session["user_id"] == null)
                return RedirectToAction("Index", "Home");
            Models.Post thePost = db.Posts.SingleOrDefault(p => p.post_id == id);
            return View(thePost);
        }

        // POST: Home/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            if (Session["user_id"] == null)
                return RedirectToAction("Index", "Home");
            Models.Post thePost = db.Posts.SingleOrDefault(p => p.post_id == id);
            try
            {
                // TODO: Add update logic here
                thePost.message = collection["message"];
                //thePost.author = collection["author"];
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(thePost);
            }
        }

        // GET: Home/Delete/5
        public ActionResult Delete(int id)
        {
            if (Session["user_id"] == null)
                return RedirectToAction("Index", "Home");
            Models.Post thePost = db.Posts.SingleOrDefault(p => p.post_id == id);
            return View(thePost);
        }

        // POST: Home/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            if (Session["user_id"] == null)
                return RedirectToAction("Index", "Home");
            Models.Post thePost = db.Posts.SingleOrDefault(p => p.post_id == id);
            try
            {
                // TODO: Add delete logic here
                db.Posts.Remove(thePost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(thePost);
            }
        }
    }
}
