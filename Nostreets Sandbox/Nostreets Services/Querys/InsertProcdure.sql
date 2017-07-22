CREATE Proc [dbo].[{0}s_Insert]
{1}
As
Begin

Declare @NewId {2}

Insert Into dbo.{3}s(
{4}
)
Values(
{5}
)
Set @NewId = SCOPE_IDENTITY()
Select @NewId
End

