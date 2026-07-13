using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class AttachmentDataManager : IAttachmentDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        // PDF/PNG/Excel, per user's request 2026-07-13 — anything else is rejected
        // with a clear message rather than silently accepted.
        private static readonly HashSet<string> AllowedContentTypes = new HashSet<string>
        {
            "application/pdf",
            "image/png",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/vnd.ms-excel"
        };

        public AttachmentDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public AttachmentListResponse GetByEntity(GetAttachmentsByEntityRequest request)
        {
            var response = new AttachmentListResponse();
            try
            {
                response.items = _unitOfWork.AttachmentRepository
                    .GetByEntity(request.EntityType, request.EntityId)
                    .Select(ToDataContract)
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public UploadAttachmentResponse Upload(UploadAttachmentRequest request)
        {
            var response = new UploadAttachmentResponse { RequestId = request.RequestId };
            try
            {
                if (!AllowedContentTypes.Contains(request.ContentType))
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = "Loại file không được hỗ trợ — chỉ nhận PDF, PNG hoặc Excel." });
                    return response;
                }

                byte[] fileBytes = Convert.FromBase64String(request.FileContentBase64);

                var config = _unitOfWork.AttachmentConfigRepository.Get();
                long maxBytes = (long)config.MaxFileSizeMb * 1024 * 1024;
                if (fileBytes.LongLength > maxBytes)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = $"File vượt quá giới hạn cho phép ({config.MaxFileSizeMb}MB)." });
                    return response;
                }

                var entity = new Attachment
                {
                    EntityType = request.EntityType,
                    EntityId = request.EntityId,
                    FileName = request.FileName,
                    ContentType = request.ContentType,
                    FileSize = fileBytes.Length,
                    FileContent = fileBytes,
                    IndActivo = true,
                    UtilizadorCriacao = request.UserId,
                    DataCriacao = DateTime.Now
                };

                _unitOfWork.AttachmentRepository.Add(entity);
                _unitOfWork.Commit();

                response.id = entity.Id;
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public Attachment GetForDownload(int id)
        {
            return _unitOfWork.AttachmentRepository.Get(id);
        }

        // Cho phép xoá file đính kèm lỡ upload sai (2026-07-13, user yêu cầu) — không
        // kiểm tra Estado của entity gốc ở đây: widget đính kèm ở frontend chỉ hiện
        // khi entity gốc còn DRAFT (xem app-attachment-upload usages), nên nút xoá
        // trên UI vốn đã không thể bấm được sau khi entity gốc đã duyệt. Attachment
        // không phải bản ghi tài chính (chỉ là chứng từ hỗ trợ) nên không thuộc phạm
        // vi nguyên tắc "không xoá được sau khi Approved" (xem CLAUDE.md §9).
        public ResponseBaseDataContract Delete(DeleteAttachmentRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                var entity = _unitOfWork.AttachmentRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = "Không tìm thấy file đính kèm." });
                    return response;
                }

                entity.IndActivo = false;
                entity = (Attachment)_utils.UpdateDetailsToEntity(entity);
                _unitOfWork.AttachmentRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        private static AttachmentDataContract ToDataContract(Attachment entity)
        {
            return new AttachmentDataContract
            {
                id = entity.Id,
                fileName = entity.FileName,
                contentType = entity.ContentType,
                fileSize = entity.FileSize,
                dataCriacao = entity.DataCriacao
            };
        }
    }
}
