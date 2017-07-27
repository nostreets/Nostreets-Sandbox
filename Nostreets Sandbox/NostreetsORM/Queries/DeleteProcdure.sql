CREATE Proc [dbo].[{0}s_Delete]
@{1} {2}
As
Begin

Delete {0}s Where {1} = @{1}

{3}

End