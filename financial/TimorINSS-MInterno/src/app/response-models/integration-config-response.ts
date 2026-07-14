import { ResponseBase } from './utils-response';

export interface IntegrationConfigItem {
  benefitApiEnabled: boolean;
}

export interface IntegrationConfigResponse extends ResponseBase {
  item: IntegrationConfigItem;
}
