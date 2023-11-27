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
using Services;

namespace Pages;
public class ResetPasswordModel : PageModel
{
    private readonly ApplicationUserManager _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;

    public ResetPasswordModel(ApplicationUserManager userManager, SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    [BindProperty]
    public string UserId { get; set; }
    [BindProperty]
    public string Token { get; set; }

    public class InputModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string Password { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = "";
    }

    public void OnGet(string userId, string token)
    {
        UserId = userId;
        Token = token;
        Console.WriteLine($"a{UserId}");
        Console.WriteLine($"a{Token}");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            Console.WriteLine($"b{UserId}");
            Console.WriteLine($"b{Token}");
            var user = _userManager.Users.FirstOrDefault(x => x.Id == UserId);

            var result = await _userManager.ResetPasswordAsync(user, Token, Input.ConfirmPassword);
            if (!result.Succeeded) return BadRequest(result.ToString());

            await _signInManager.SignInAsync(user, true);

            return LocalRedirect("/");
        }

        // If we got this far, something failed, redisplay form
        return Page();
    }
}
