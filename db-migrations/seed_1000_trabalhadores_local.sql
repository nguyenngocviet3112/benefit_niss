-- LOCAL-ONLY: seed 1 test employer with 1000 active employees, to test the
-- Declaração de Remuneração screen (Contribuição module) at scale after the
-- N+1 query fix in GetDeclaracaoByEntidadeAndFilter (2026-07-24).
-- NOT idempotent -- creates NEW rows every run. Run once against a fresh local
-- Docker DB (TimorINSSModuloContribuicoes_ProdDev).

SET NOCOUNT ON;
SET XACT_ABORT ON; -- se qualquer statement falhar, faz rollback de tudo (evita dados órfãos parciais)
BEGIN TRANSACTION;

DECLARE @admin INT = 23;
DECLARE @sexoMasculino INT = (SELECT idDominio FROM DOMINIO WHERE dominio='SEXO' AND valor=1);
DECLARE @nacionalidadeTL INT = (SELECT idDominio FROM DOMINIO WHERE dominio='NACIONALIDADE' AND valor=1);

DECLARE @qtd INT = 200;

-- 1. Entidade empregadora de teste
INSERT INTO ENTIDADEEMPREGADORA (nome, NISS, TIN, dtInscricao, situacInscricao, dataInicioActiv, dataInicioTrabServico, numTrabalhador,
    entidade_natJuridica_fk, entidade_actEconomica_fk, entidade_sectorAct_fk, dtHoraUltimoAcesso, flagImportado, utilizadorCriacao, dataCriacao)
VALUES (N'Test Company 200 Employees', '900099999', '9099999', '2020-01-01', 'A', '2020-01-01', '2020-01-01', 200,
    1, 1, 1, GETDATE(), 0, @admin, GETDATE());
DECLARE @entidadeTeste INT = SCOPE_IDENTITY();
PRINT 'EntidadeEmpregadora teste id=' + CAST(@entidadeTeste AS VARCHAR);

-- 2. 200 Trabalhador, via tally numbers CTE (bulk, 1 statement)
;WITH Tally AS (
    SELECT TOP (200) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
    FROM sys.all_objects a CROSS JOIN sys.all_objects b
)
INSERT INTO TRABALHADOR (nome, TIN, naturalidade, dataNasc, nomeMae, nomePai, indDescNomeMae, indDescNomePai,
    flagImportado, sexoTrabalhador, nacionalidadeTrabalhador, interno, utilizadorCriacao, dataCriacao, NISS)
SELECT
    N'Test Worker ' + CAST(n AS VARCHAR),
    N'TIN-TEST-' + RIGHT('0000000' + CAST(n AS VARCHAR), 7),
    N'Dili',
    DATEADD(YEAR, -20 - (n % 30), '2026-01-01'),
    N'Test Mother',
    N'Test Father',
    0, 0,
    0,
    @sexoMasculino,
    @nacionalidadeTL,
    0,
    @admin, GETDATE(),
    N'900199' + RIGHT('0000' + CAST(n AS VARCHAR), 4)
FROM Tally;

-- 3. Relaciona os 200 Trabalhador recém-criados (por NISS, únicos) com a Entidade de teste
;WITH Tally AS (
    SELECT TOP (200) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
    FROM sys.all_objects a CROSS JOIN sys.all_objects b
)
INSERT INTO RELENTIDADETRABALHADOR (entidade_fk, trabalhador_fk, tipoContrato, naturezaContrato, leiLabAplicavel,
    horasSemana, diasSemana, dtIniVincTrabalhador, dtIniFimTrabalhador, funcPublico, flagImportado,
    utilizadorCriacao, dataCriacao, regime_fk, profissao)
SELECT
    @entidadeTeste,
    t.idTrabalhador,
    4, 5, 8, 40, 5,
    '2025-01-01', NULL, 0, 0,
    @admin, GETDATE(), 23, 52
FROM Tally
JOIN TRABALHADOR t ON t.NISS = N'900199' + RIGHT('0000' + CAST(Tally.n AS VARCHAR), 4);

PRINT 'RELENTIDADETRABALHADOR criados: ' + CAST(@@ROWCOUNT AS VARCHAR);

COMMIT TRANSACTION;

SELECT @entidadeTeste AS EntidadeTesteId;
