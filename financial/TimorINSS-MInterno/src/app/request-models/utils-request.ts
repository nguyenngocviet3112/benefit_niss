export class FilterRequest {
  index?: number;
  rows?: number;
  dateFilterBegin?: Date;
  dateFilterEnd?: Date;
  filterField?: string;
  filterBy?: string;
  orderBy? : string;
  orderDirection? : OrderDirectionEnum;
  filter? : FilterRequest;
}

export enum OrderDirectionEnum {
  ascending = 1,
  descending = 2,
}
