-- =============================================
-- UpdatePasswordHash.sql
-- Cập nhật password từ plain text sang SHA256 hash
-- =============================================

USE FootballManagementDB;
GO

-- Drop hàm hash nếu tồn tại
IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'FN' AND name = 'fn_HashPassword')
    DROP FUNCTION dbo.fn_HashPassword
GO

-- Tạo hàm hash password
CREATE FUNCTION dbo.fn_HashPassword(@Password NVARCHAR(255))
RETURNS NVARCHAR(64)
AS
BEGIN
    RETURN CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', @Password), 2)
END
GO

-- Update password admin và user từ '123' thành hash
UPDATE Users 
SET PasswordHash = dbo.fn_HashPassword('123')
WHERE Username IN ('admin', 'user')
GO

-- Kiểm tra kết quả
PRINT '✅ Cập nhật password thành công!'
SELECT UserID, Username, Email, FullName, PasswordHash, RoleID, IsActive 
FROM Users 
ORDER BY UserID
GO
