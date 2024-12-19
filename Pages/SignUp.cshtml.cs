// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Data;

namespace Pages;
public class SignUpModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;

    public SignUpModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    [TempData]
    public string ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "You must enter a first name")]
        public string GivenName { get; set; }

        public string FamilyName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(string returnUrl = null)
    {
        if (HttpContext.User.Identity.IsAuthenticated) return Redirect("/");

        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        returnUrl ??= Url.Content("~/");

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ReturnUrl = returnUrl;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            var user = new ApplicationUser()
            {
                GivenName = Input.GivenName,
                FamilyName = Input.FamilyName,
                UserName = Input.Email,
                Email = Input.Email,
            };
            // todo: check is valid
            // if (!user.IsValid())
            // {
            //     ModelState.AddModelError("error", "Username");
            //     return Page();
            // }
            var result = await _userManager.CreateAsync(user, Input.Password);
            await _signInManager.SignInAsync(user, true);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return LocalRedirect(returnUrl);
            }
            else
            {
                ModelState.AddModelError("error", String.Join("\n", result.Errors.Select(x => x.Description)));
                return Page();
            }
        }

        // If we got this far, something failed, redisplay form
        return Page();
    }
}
