using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using PagedList;

namespace WebApplication1.Controllers
{
    public class MoviesController : Controller
    {
        private MovieDBContext db = new MovieDBContext();
        // GET: Movies
        public ActionResult Index(string MovieGenre, string SearchString, int? page, string dateSortOrder = null, string titleSortOrder = null)
        {
            ViewBag.CurrentDateSort = dateSortOrder;
            ViewBag.CurrentTitleSort = titleSortOrder;
            ViewBag.CurrentFilter = SearchString;
            ViewBag.CurrentGenre = MovieGenre;
            ViewBag.TitleSortParam = titleSortOrder == "title"? "title_desc" : "title";
            ViewBag.DateSortParam = dateSortOrder == "date"? "date_desc" : "date";
            var genres = new List<string>();
            var genresQuery = from d in db.Movies orderby d.Genre select d.Genre;
            genres.AddRange(genresQuery.Distinct());
            ViewBag.movieGenre = new SelectList(genres);
            var movies = from m in db.Movies select m;
            if (!String.IsNullOrEmpty(SearchString))
            {
                movies = movies.Where(s => s.Title.Contains(SearchString));
            }
            if (!String.IsNullOrEmpty(MovieGenre)) {
                movies = movies.Where(x => x.Genre == MovieGenre);
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            var pager = movies.OrderByDescending(m => m.Title).ToPagedList(pageNumber, pageSize);
            switch (titleSortOrder){
                case "title_desc":
                    pager = movies.OrderByDescending(m => m.Title).ToPagedList(pageNumber, pageSize);
                    break;
                case "title":
                    pager = movies.OrderBy(m => m.Title).ToPagedList(pageNumber, pageSize);
                    break;
                default:
                    break;
            }
            switch (dateSortOrder) {
                case "date":
                    pager = movies.OrderBy(m => m.ReleaseDate).ToPagedList(pageNumber, pageSize);
                    break;
                case "date_desc":
                    pager = movies.OrderByDescending(m => m.ReleaseDate).ToPagedList(pageNumber, pageSize);
                    break;
                default:
                    break;
            }
            return View(pager);
        }

        // GET: Movies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // GET: Movies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.Movies.Add(movie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Movie movie = db.Movies.Find(id);
            db.Movies.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
