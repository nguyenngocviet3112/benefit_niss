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
    requestId?: string;
}
