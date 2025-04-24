// Packages
using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Moq;

// Refs 
using deckster.database;
using deckster.services;
using deckster.services.commands;
using deckster.cqs;

namespace deckster.__Tests__;

public class AuthTest
{

  private readonly Mock<IJWTService> _mockJWT;
  private readonly Mock<IHashService> _mockHash;
  private readonly Mock<IConfiguration> _mockConfig;
  private readonly Mock<IDataContext> _mockDataContext;

  private readonly AuthService _auth;

  public AuthTest ()
  {
    _mockConfig = new();
    _mockHash = new();
    _mockJWT = new();
    _mockDataContext = new();

    _auth = new(_mockDataContext.Object,_mockHash.Object,_mockConfig.Object,_mockJWT.Object);
  }

  [Fact]
  [Description("Happy path")]
  public void RegisterValidModel()
  {
    // Arrange
    string storedProcName = "RegisterUserTransaction";
    using SqlConnection con = new();

    // Mock the command that would be created in the Execute method

    _mockDataContext.Setup(s => s.CreateConnection()).Returns(() => con);
    _mockDataContext.Setup(s => s.ExecuteNonQuery(
          It.Is<string>(q => q == storedProcName),
          It.IsAny<SqlCommand>()
          )).Returns(1);

    RegisterUserCommand command = new("Victor", "VictorDoe@test.com", "SIMPLEtest55=");


    CommandResult result = _auth.Execute(command);


    // Assert
    Assert.True(result.IsSuccess);
    _mockHash.Verify(s=>s.HashPassword("SIMPLEtest55="),Times.Once);
    _mockDataContext.Verify(s=>s.CreateConnection(),Times.Once);

  }
}

