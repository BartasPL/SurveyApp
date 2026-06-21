using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SurveyApp.Data;
using SurveyApp.Models;

namespace SurveyApp.Controllers
{
    [Authorize]
    public class SurveyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SurveyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Wyswietlanie listy ankiet
        public IActionResult Index()
        {
            // Pobieranie wszystkich ankiet z bazy
            var ankiety = _context.Surveys.ToList();
            return View(ankiety);
        }

        // Widok do dodawania (tylko dla ankietera)
        [Authorize(Roles = "Ankieter")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Ankieter")]
        public IActionResult Create(string title, string description, List<string> options)
        {
            var myUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Survey nowaAnkieta = new Survey();
            nowaAnkieta.Title = title;
            nowaAnkieta.Description = description;
            nowaAnkieta.CreatorId = myUserId;

            foreach (var opcja in options)
            {
                if (opcja != null && opcja != "")
                {
                    Option o = new Option();
                    o.Text = opcja;
                    nowaAnkieta.Options.Add(o);
                }
            }

            _context.Surveys.Add(nowaAnkieta);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Widok glosowania
        public IActionResult Vote(int id)
        {
            var ankieta = _context.Surveys
                .Include(x => x.Options)
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (ankieta == null) 
            {
                return RedirectToAction("Index");
            }

            string aktualnyUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var sprawdzGlos = _context.Votes
                .Where(v => v.Option.SurveyId == id && v.UserId == aktualnyUser)
                .FirstOrDefault();

            if (sprawdzGlos != null)
            {
                return RedirectToAction("Results", new { id = id });
            }

            return View(ankieta);
        }

        [HttpPost]
        public IActionResult SubmitVote(int optionId)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wybranaOpcja = _context.Options.Find(optionId);
            
            bool czyJuzGlosowal = _context.Votes.Any(x => x.Option.SurveyId == wybranaOpcja.SurveyId && x.UserId == user);

            if (czyJuzGlosowal == false)
            {
                Vote nowyGlos = new Vote();
                nowyGlos.OptionId = optionId;
                nowyGlos.UserId = user;

                _context.Votes.Add(nowyGlos);
                _context.SaveChanges();
            }

            return RedirectToAction("Results", new { id = wybranaOpcja.SurveyId });
        }

        // Wyniki ankiety
        public IActionResult Results(int id)
        {
            var ankietaZWynikami = _context.Surveys
                .Include(s => s.Options)
                .ThenInclude(o => o.Votes)
                .FirstOrDefault(s => s.Id == id);

            return View(ankietaZWynikami);
        }

        public async Task<IActionResult> DajMiRole([FromServices] Microsoft.AspNetCore.Identity.UserManager<Microsoft.AspNetCore.Identity.IdentityUser> userManager)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Content("Musisz byc najpierw zalogowany!");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await userManager.AddToRoleAsync(user, "Ankieter");
                return Content("Gratulacje, jestes teraz Ankieterem! Zrestartuj logowanie (wyloguj i zaloguj ponownie).");
            }
            return Content("Nie znaleziono uzytkownika w bazie!");
        }
    }
}