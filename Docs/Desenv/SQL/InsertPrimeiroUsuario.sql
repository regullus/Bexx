USE [Bexx]
GO

INSERT INTO [dbo].[usuario]
           ([idUsuarioDados]
           ,[idEmpresa]
           ,[idGrupo]
           ,[idUsuarioSituacao]
           ,[idArea]
           ,[idPais]
           ,[idSexo]
		   ,idIdioma
           ,[criacao]
           ,[atualizacao]
		   ,[nome]
           ,[senha]
           ,[dataExpiracaoSenha]
           ,[avatar]
           ,[termoUsoEm]
           ,[googleAuthenticatorSecretKey]
           ,[login]
           ,[normalizedLogin]
           ,[email]
           ,[normalizedEmail]
           ,[emailConfirmado]
           ,[passwordHash]
           ,[securityStamp]
           ,[concurrencyStamp]
           ,[celular]
           ,[celularConfirmado]
           ,[twoFactorEnabled]
           ,[lockoutEnd]
           ,[lockoutEnabled]
           ,[accessFailedCount])
     VALUES
          (1
           ,1
           ,1
           ,1
           ,1
           ,1
           ,1
           ,1
           ,20210701
           ,20210701
		    ,'Marcus Rabello'
           ,'013017010017010017088065'
           ,'20500101'
           ,''
           ,'20210701'
           ,''
           ,'rabello'
           ,'RABELLO'
           ,'rabellomc@gmail.com'
           ,'RABELLOMC@GMAIL.COM'
           ,1
           ,'AQAAAAEAACcQAAAAEJYrkwUY43xl2cJn7qQMeGG4RTbhHtlURBbG56b02xGv8FXenkQsGJYqShM0Nuchrw=='
           ,'7JVVAR4FGXRF754QCSYUYTPTDRNNAAOL'
           ,'d76fcc9f-a3f4-4d3d-9454-661a276b0e4a'
           ,'5511957326666'
           ,1
           ,0
           ,null
           ,1
           ,0)
GO


select * from usuario





