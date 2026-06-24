/*

using Microsoft.AspNetCore.Identity.Data;
using Project.Api.Dtos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();

var app = builder.Build();

List<UserDto> users = [
    new(1,"gopika@gmail.com","123gop"),
    new(2,"gop@gmail.com", "1234")
];

app.MapGet("/users", () => users);

app.MapGet("/users/{id}", (int id) =>
{
    var user = users.Find(user => user.Id == id);

    return user is null? Results.NotFound() : Results.Ok(user);
});


app.MapPost("/users", (CreateUserDto newUser) =>
{
    if (string.IsNullOrEmpty(newUser.email))
    {
        return Results.BadRequest("Email is required");
    }

    UserDto user = new(
        users.Count +1,
        newUser.email,
        newUser.pass
    );

    users.Add(user);

    return Results.Created($"/users/{user.Id}", user);
});


app.MapPut("/users/{id}", (int id, CreateUserDto updUser) =>
{
    //find location
    //update that location

    var index = users.FindIndex(user => user.Id==id);

    if (index == -1)
    {
        return Results.NotFound();
    }

    users[index] = new UserDto(
        id,
        updUser.email,
        updUser.pass
    );

    return Results.NoContent();
});


app.MapDelete("/users/{id}", (int id) =>
{
    users.RemoveAll(user => user.Id==id);

    return Results.NoContent();
});



//REGISTER LOGIN
app.MapPost("/users/register", (CreateUserDto newUser) =>
{
    var exist = users.Find(user => user.email==newUser.email);
    if (exist is not null)
    {
        return Results.BadRequest("user already exists");
    }


    UserDto user = new(
        users.Count + 1,
        newUser.email,
        newUser.pass
    );

    users.Add(user);

    return Results.Ok(new UserResponse(user.Id, user.email));

});


app.MapPost("users/login", (CreateUserDto login) => 
{
    var user = users.Find(user => user.email==login.email && user.pass==login.pass);

    if (user is null)
    {
        return Results.Unauthorized();
    }

    return Results.Ok(new UserResponse(user.Id, user.email));
});

app.Run();

*/