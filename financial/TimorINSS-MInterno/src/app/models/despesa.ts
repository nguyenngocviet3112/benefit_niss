export interface Despesa
{
  id: number;
  idOrcamentoRegistoAprovado: number;
  tarefaAtivoFK: number;
  departamentoFk?: number;
  centroCustoFk: number;
  tipoContaFk: number;
  codigoContaFk: number;
  agrupamentoConfigFk: number;
  institutionId: number;
  actidadeFk: number;
  economicFk: number;
  funcionalFk: number;
  descricao: string;
  valor: number|undefined;
}


export interface ValoresDespesaRegistada
{
  descricao: string;
  valorOrcamentado: number;
  valorExecutado: number;
  valorCabimentado: number;
  valorAutorizado: number;
}