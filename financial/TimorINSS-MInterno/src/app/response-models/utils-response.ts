import { SelectDescription } from "../models/utils";

export interface SelectDescriptionResponse
{
    selects: SelectDescription[];
}

export interface ResponseError {
    errorCode: string;
    errorMessage: string;
}

export interface ResponseBase {
    errors?: ResponseError[];
    // Không chặn hành động (khác errors) — thông báo bút toán kế toán vừa được
    // tự sinh (kiểm tra lại nếu sai) hoặc bị bỏ qua vì thiếu cấu hình (2026-07-13).
    warnings?: string[];
    requestId?: string;
}

// Dùng chung cho mọi endpoint "Xuất Excel" (StringFileReponse phía backend) —
// file là chuỗi base64, giải mã bằng base64ToArrayBuffer/blobToSaveAs (utils.ts).
export interface StringFileResponse extends ResponseBase {
    file: string;
}
