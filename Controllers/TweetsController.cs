using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tweeter.Data;
using Tweeter.Models;

namespace Tweeter.Controllers
{
    public class TweetsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager { get; }

        private IQueryable<Tweet> authorizedTweets;

        public TweetsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

            authorizedTweets = (from t in _context.Tweet join user in _context.Users on t.OwnerID equals user.Id where user.Email == User.Identity.Name select t);
        }

        // GET: Tweets
        public async Task<IActionResult> Index()
        {
            return View(await authorizedTweets.ToListAsync());
        }

        // GET: Tweets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tweet = await authorizedTweets
                .SingleOrDefaultAsync(m => m.ID == id);
            if (tweet == null)
            {
                return NotFound();
            }

            return View(tweet);
        }

        // GET: Tweets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tweets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Text,CreatedOn")] Tweet tweet)
        {
            if (ModelState.IsValid)
            {
                tweet.OwnerID = _userManager.GetUserId(User);
                _context.Add(tweet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tweet);
        }

        // GET: Tweets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tweet = await authorizedTweets.SingleOrDefaultAsync(m => m.ID == id);
            if (tweet == null)
            {
                return NotFound();
            }
            return View(tweet);
        }

        // POST: Tweets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Text,CreatedOn")] Tweet tweet)
        {
            if (id != tweet.ID)
            {
                return NotFound();
            }

            var authorizedTweet = await authorizedTweets.SingleOrDefaultAsync(m => m.ID == id);
            if (null == authorizedTweet)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tweet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TweetExists(tweet.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tweet);
        }

        // GET: Tweets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tweet = await authorizedTweets
                .SingleOrDefaultAsync(m => m.ID == id);
            if (tweet == null)
            {
                return NotFound();
            }

            return View(tweet);
        }

        // POST: Tweets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tweet = await authorizedTweets.SingleOrDefaultAsync(m => m.ID == id);
            _context.Tweet.Remove(tweet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TweetExists(int id)
        {
            return authorizedTweets.Any(e => e.ID == id);
        }
    }
}
