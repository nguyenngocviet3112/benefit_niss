export interface SaveFunctionalClassificationRequest {
  id: number;
  codigo: string;
  designacao: string;
  parentFk?: number;
}

export interface DeactivateFunctionalClassificationRequest {
  id: number;
}
