# JWT Authentication and Swagger Configuration Guide

## Fixed Issues

### 1. JWT Authentication Configuration ✅
**Problem**: `RequireHttpsMetadata = false` and issuer/audience mismatch
**Solution**: 
- Conditional HTTPS metadata based on environment
- Added multiple valid issuers/audiences for development
- Proper token validation with clock skew

### 2. Swagger Bearer Token Configuration ✅
**Problem**: Incorrect security scheme definition
**Solution**:
- Added `BearerFormat = "JWT"`
- Improved description with clear instructions
- Proper scheme configuration

### 3. Role Claims ✅
**Problem**: AdminService expects role claims but JWT didn't include them
**Solution**: Added `ClaimTypes.Role, "User"` to JWT token generation

## Correct Program.cs Configuration

```csharp
// JWT Authentication Setup
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = builder.Environment.IsDevelopment() ? false : true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        // Allow development URLs for testing
        ValidIssuers = new[] 
        { 
            jwtSettings["Issuer"],
            "http://localhost:5002",  // Add your service port
            "https://localhost:5002",
            "http://192.168.1.1:5002"
        },
        ValidAudiences = new[] 
        { 
            jwtSettings["Audience"],
            "http://localhost:5002",  // Add your service port
            "https://localhost:5002",
            "http://192.168.1.1:5002"
        }
    };
});

builder.Services.AddAuthorization();

// Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Your API", 
        Version = "v1"
    });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Middleware Order (CRITICAL)
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();  // MUST come before Authorization
app.UseAuthorization();
app.MapControllers();
```

## [Authorize] and [AllowAnonymous] Usage Examples

### 1. Controller Level Authorization
```csharp
[ApiController]
[Route("api/admin")]
[Authorize]  // All endpoints require authentication
public class AdminController : ControllerBase
{
    [HttpGet("profile")]
    public IActionResult GetProfile() 
    {
        // Requires valid JWT token
        return Ok("User profile");
    }
    
    [HttpPost("login")]
    [AllowAnonymous]  // Override controller-level [Authorize]
    public IActionResult Login() 
    {
        // No authentication required
        return Ok("Login endpoint");
    }
}
```

### 2. Role-Based Authorization
```csharp
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    [HttpGet("users")]
    [Authorize(Roles = "Admin")]  // Only Admin role can access
    public IActionResult GetAllUsers() 
    {
        return Ok("All users");
    }
    
    [HttpGet("profile")]
    [Authorize(Roles = "Admin,User")]  // Admin OR User can access
    public IActionResult GetProfile() 
    {
        return Ok("User profile");
    }
}
```

### 3. Policy-Based Authorization
```csharp
// In Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => 
        policy.RequireRole("Admin"));
    options.AddPolicy("RequireMinimumAge", policy => 
        policy.RequireClaim(ClaimTypes.DateOfBirth, "1990-01-01"));
});

// In Controller
[HttpGet("admin-only")]
[Authorize(Policy = "RequireAdminRole")]
public IActionResult AdminOnlyEndpoint()
{
    return Ok("Admin only");
}
```

### 4. Mixed Authorization Examples
```csharp
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    // Public endpoint - no authentication required
    [HttpPost("register")]
    [AllowAnonymous]
    public IActionResult Register() { return Ok(); }
    
    // Authentication required but any role
    [HttpGet("profile")]
    [Authorize]
    public IActionResult GetProfile() { return Ok(); }
    
    // Specific role required
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteUser(int id) { return Ok(); }
    
    // Multiple roles allowed
    [HttpGet("admin-data")]
    [Authorize(Roles = "Admin,Moderator")]
    public IActionResult GetAdminData() { return Ok(); }
}
```

## Common 401 Unauthorized Mistakes in Swagger

### 1. **Incorrect Token Format**
❌ Wrong: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9`
✅ Correct: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9`

### 2. **Expired Token**
- Check token expiration in JWT payload
- Ensure `ValidateLifetime = true` in configuration

### 3. **Invalid Issuer/Audience**
- Token issuer must match configured ValidIssuer
- Token audience must match configured ValidAudience

### 4. **Missing Role Claims**
- Admin endpoints with `[Authorize(Roles = "Admin")]` need role claims
- Add `new Claim(ClaimTypes.Role, "Admin")` to token generation

### 5. **Middleware Order**
```csharp
// WRONG ORDER - will cause 401
app.UseAuthorization();
app.UseAuthentication();

// CORRECT ORDER
app.UseAuthentication();  // First
app.UseAuthorization();   // Second
```

### 6. **HTTPS Metadata in Development**
```csharp
// For development
options.RequireHttpsMetadata = false;

// For production
options.RequireHttpsMetadata = true;
```

## Testing in Swagger

1. **Get Token**: Call login endpoint (e.g., `/api/users/auth/verify-otp`)
2. **Copy Token**: Copy the `token` value from response
3. **Authorize in Swagger**: 
   - Click "Authorize" button (🔒)
   - Enter: `Bearer YOUR_TOKEN_HERE`
   - Click "Authorize"
4. **Test Protected Endpoints**: Now you can access `[Authorize]` endpoints

## Quick Troubleshooting Checklist

- [ ] Token has "Bearer " prefix
- [ ] Token is not expired
- [ ] Issuer/Audience match configuration
- [ ] Role claims exist for role-based endpoints
- [ ] Middleware order is correct
- [ ] HTTPS metadata is properly configured
- [ ] Swagger security definition includes BearerFormat
