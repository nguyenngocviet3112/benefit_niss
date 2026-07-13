import { ResponseBase } from './utils-response';

export interface AttachmentItem {
  id: number;
  fileName: string;
  contentType: string;
  fileSize: number;
  dataCriacao: string;
}

export interface AttachmentListResponse extends ResponseBase {
  items: AttachmentItem[];
}

export interface UploadAttachmentResponse extends ResponseBase {
  id: number;
}
