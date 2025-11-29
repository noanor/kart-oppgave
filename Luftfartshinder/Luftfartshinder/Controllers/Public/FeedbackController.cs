using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using Luftfartshinder.Models.ViewModel.User;

namespace Luftfartshinder.Controllers.Public
{
    public class FeedbackController : Controller
    {
        // Feedback-side: iPad-vennlig layout
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Feedback()
        {
            ViewData["LayoutType"] = "ipad";
            var model = new FeedbackViewModel
            {
                Name = string.Empty,
                Email = string.Empty,
                Message = string.Empty
            };
            
            // Set Q&A data
            ViewBag.QandA = GetQandAData();
            
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Feedback(FeedbackViewModel model)
        {
            ViewData["LayoutType"] = "ipad";
            if (!ModelState.IsValid)
            {
                ViewBag.QandA = GetQandAData();
                return View(model);
            }

            // Here you can save the feedback to a database or send an email
            // For now, we'll just show a success message

            // Generate a short 7-char reference (A-Z, 2-9; avoiding ambiguous chars)
            const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var randomBytes = RandomNumberGenerator.GetBytes(7);
            var refChars = new char[7];
            for (int i = 0; i < refChars.Length; i++)
            {
                refChars[i] = alphabet[randomBytes[i] % alphabet.Length];
            }
            var referenceNumber = new string(refChars);

            TempData["SuccessMessage"] = $"Thank you for your feedback! We appreciate your input. Your reference number is {referenceNumber}. Please include this reference if you contact customer support.";
            
            return RedirectToAction("Index");
        }

        private List<QandAItem> GetQandAData()
        {
            return new List<QandAItem>
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
        }
    }
}

