// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Data;
using Services;

namespace Pages;
public class RequestResetPasswordModel : PageModel
{
    private readonly ApplicationUserManager _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;

    public RequestResetPasswordModel(ApplicationUserManager userManager, SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Please enter an email")]
        [EmailAddress]
        public string Email { get; set; } = "";
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Email == Input.Email);
            if (user == null) goto ok; // we don't want to say not found

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Pretend we are sending an email
            var query = new QueryBuilder
            {
                { "UserId", user.Id },
                { "Token", token }
            };

            var url = "/ResetPassword" + query.ToString();
            Console.WriteLine($"Password reset token is: {url}");

        ok:
            return LocalRedirect("/PasswordResetRequested");
        }

        // If we got this far, something failed, redisplay form
        return Page();
    }
}
