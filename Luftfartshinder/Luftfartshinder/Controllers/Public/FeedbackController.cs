using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using Luftfartshinder.Models.ViewModel.User;

namespace Luftfartshinder.Controllers
{
    /// <summary>
    /// Controller for the feedback page. Displays the feedback form and handles submissions
    /// </summary>
    public class FeedbackController : Controller
    {
        private const string IpadLayoutType = "ipad";
        private const string SuccessTempDataKey = "SuccessMessage";
        private const int ReferenceLength = 7;
        private static readonly string ReferenceAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        /// <summary>
        /// Predefined Q&A shown on the feedback page.
        /// </summary>
        private static readonly IReadOnlyList<QandAItem> CachedQandA = new List<QandAItem>
        {
            new QandAItem
            {
                Question = "How do I submit feedback?",
                Answer = "You can send feedback by filling out the form on this page. Please describe the issue or suggestion as clearly as possible, and include a map reference or screenshot if relevant."
            },
            new QandAItem
            {
                Question = "What type of feedback can I submit?",
                Answer = "You can report data errors, missing information, technical issues, or share suggestions for improving the system or user experience."
            },
            new QandAItem
            {
                Question = "How long does it take to receive a response?",
                Answer = "The usual response time is 3–5 business days. You will receive an email confirmation when your feedback has been received and when it has been processed."
            },
            new QandAItem
            {
                Question = "Can I edit or delete my feedback after submitting it?",
                Answer = "Yes, please contact us using the email address provided in your confirmation message. Include your reference number so we can locate your submission."
            },
            new QandAItem
            {
                Question = "Are my feedback submissions visible to others?",
                Answer = "No, all feedback is confidential and used internally to improve the system and data quality."
            }
        };

        /// <summary>
        /// Show the feedback form 
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Feedback()
        {
            SetIpadLayout();
            ViewBag.QandA = CachedQandA;
            var model = new FeedbackViewModel();
            return View(model);
        }

        /// <summary>
        /// Receive feedback submissions
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Feedback(FeedbackViewModel model)
        {
            SetIpadLayout();

            if (!ModelState.IsValid)
            {
                ViewBag.QandA = CachedQandA;
                return View(model);
            }
            
            var referenceNumber = GenerateReference(ReferenceLength);

            TempData[SuccessTempDataKey] = $"Thank you for your feedback! We appreciate your input. Your reference number is {referenceNumber}. Please include this reference if you contact customer support.";
            
            return RedirectToAction("Index");
        }
        
        /// <summary>
        /// Set the layout type used by the view (ipad).
        /// </summary>
        private void SetIpadLayout()
        {
            ViewData["LayoutType"] = IpadLayoutType;
        }

        /// <summary>
        /// Create a short, secure reference string of the given length.
        /// </summary>
        /// <param name="length">Requested length of the reference.</param>
        /// <returns>Generated reference string.</returns>
        private string GenerateReference(int length)
        {
            if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));

            var bytes = RandomNumberGenerator.GetBytes(length);
            var chars = new char[length];

            for (int i = 0; i < length; i++)
            {
                chars[i] = ReferenceAlphabet[bytes[i] % ReferenceAlphabet.Length];
            }

            return new string(chars);
        }
    }
}