---
page_type: sample
languages:
  - C#
  - .Net core
products:
  - azure-active-directory
description: "A sample to demonstrate how to use a captcha service during sign-up in Azure AD B2C user flows"
urlFragment: active-directory-b2c-sign-up-user-flow-captcha
---


# active-directory-b2c-user-flow-captcha

## Contents

| File/folder                 | Description                                |
| --------------------------- | ------------------------------------------ |
| [Assets/selfAsserted.html](selfAsserted.html)      | Sample custom HTML and JS script file for user flow.     |
| [Assets](selfAsserted.html)      | Contains UI/UX assets used by the user flows.    |
| [AzureFunction.cs](AzureFunction.cs)      | Sample source code for HTTP trigger.    |
| `README.md`                 | This README file.                          |
| `.gitignore`                | Define what to ignore at commit time.      |
| `LICENSE`                   | The license for the sample.                |

## Key Concepts

Captcha services are often used in authentication scenarios to protect against bots or other automated abuse. This sample demonstrates how to use Azure AD B2C user flows to utilize a captcha service during user sign-up. 

Key components:
- **reCAPTCHA** - a captcha service for protecting against bots and other automated abuse.
- **Azure AD B2C sign-up user flow** - The sign-up experience that will be using the captcha service. Will utilize the **custom page content** and **API connectors** to integrate with the captcha service.
- **Azure Function** - API endpoint hosted by you that works in conjunction with the API connectors feature. This API is responsible for inter-mediating between the user flow and the captcha service to determine whether a user can successfully sign-up.

This same pattern can be used for other Captcha services and with other API hosting services.

## Create a user flow
This can be either be a **sign up and sign in** or a just **sign up** or user flow. [Follow these instructions.](https://docs.microsoft.com/azure/active-directory-b2c/tutorial-create-user-flows).

## Create an API key pair for reCAPTCHA V3

- Follow the [reCAPTCHA documentation](https://developers.google.com/recaptcha/intro) to create an API key pair for your site.
- Use your Azure AD B2C tenant as the **domain**: `<tenantname>.b2clogin.com`
- You will receive a **site_key** and **secret_key**. The values of these are referred to as **`CAPTCHA_SITE_KEY`** and **`CAPTCHA_SECRET_KEY`** in the sample code.


