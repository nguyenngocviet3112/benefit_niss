-- Placeholder Perfil so new accounts created via "Quản lý User & Phân quyền"
-- can actually log in. InternalLoginManager (old login code, unchanged) hard
-- rejects login for any username other than "admin" that has zero active
-- Relutilizadorperfil rows ("CurrentUserHasNoProfile") — this is entirely
-- the OLD Perfil/Funcionalidade system's gate, unrelated to the new
-- UserPermission RBAC, and out of scope to rewrite right now (user chose:
-- auto-assign a placeholder Perfil at account creation instead of touching
-- login logic). This Perfil is intentionally linked to ZERO
-- Relperfilfuncionalidade rows — it grants no old-system capability, it
-- exists purely to satisfy the legacy "perfilIds.Count == 0" check.

INSERT INTO [dbo].[Perfil] (Descricao, IndActivo, UtilizadorCriacao, DataCriacao)
VALUES (N'Conta Módulo Contabilidade (sem Perfil legado)', 1, 2, GETDATE())
GO
