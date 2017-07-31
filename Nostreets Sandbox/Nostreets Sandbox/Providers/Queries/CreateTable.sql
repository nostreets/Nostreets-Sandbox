
Declare @isTrue int = 0

Begin

CREATE TABLE [dbo].[{0}s] (
{1}
);

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'{0}s')

Begin 

Set @IsTrue = 1

End

End

Select @IsTrue