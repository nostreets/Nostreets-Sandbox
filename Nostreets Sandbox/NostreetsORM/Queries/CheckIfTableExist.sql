Declare @IsTrue int = 0 

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'{0}s')
Begin 
Set @IsTrue = 1
End

Select @IsTrue