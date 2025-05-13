import {Component, Output, EventEmitter, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {PopUpWarningComponent} from "../../componentes/pop-up-warning/pop-up-warning.component";

interface ReasonOption {
  value: string;
  label: string;
}
@Component({
  selector: 'app-pop-up-handle-invoice',
  templateUrl: './pop-up-handle-invoice.component.html',
  styleUrls: ['./pop-up-handle-invoice.component.css']
})
export class PopUpHandleInvoiceComponent {
  invoice = {
    invNo: '',
    submissionDate: '',
    paymentAmount: '',
    actualReceive: '',
    reasonOptions: [] as ReasonOption[]
  };


  selectedReason: string = '';
  statusMessage: string = '';

  @Output() approve = new EventEmitter<void>();
  @Output() reject = new EventEmitter<string>();
  @Output() cancel = new EventEmitter<void>();
  @Output() close = new EventEmitter<void>();

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
              private dialogRef: MatDialogRef<PopUpHandleInvoiceComponent>) {
    // Map the incoming data to the invoice object
    console.log(data);
    this.invoice = {
      invNo: data.paymentRef , // Default value if not provided
      submissionDate: (data.dataCriacao instanceof Date) ? data.dataCriacao.toLocaleDateString() : new Date().toLocaleDateString(), // Default value
      paymentAmount: data.total, // Format as currency
      actualReceive: data.total, // Assuming actual receive matches payment amount
      reasonOptions: data.reasonOptions
    };
    this.selectedReason =  this.invoice.reasonOptions[0].value;
  }


  onApprove(): void {
    this.statusMessage = 'Payment approved successfully!';
    this.approve.emit();
    setTimeout(() => {
      this.closePopup();
      this.dialogRef.close(true);
    }, 1500); // Close after 1.5 seconds
  }

  onReject(): void {
    if (this.selectedReason === 'Choose a reason') {
      this.statusMessage = 'Please select a reason for rejection.';
      return;
    }
    this.statusMessage = `Payment rejected: ${this.selectedReason}`;
    this.reject.emit(this.selectedReason);
    setTimeout(() => {
      this.closePopup();
      this.dialogRef.close(true);
    }, 1500); // Close after 1.5 seconds
  }

  onCancel(): void {
    this.statusMessage = 'Action canceled.';
    this.cancel.emit();
    this.dialogRef.close(true);
  }

  onClose(): void {
    this.statusMessage = 'Popup closed.';
    this.close.emit();
  }

  onReasonChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    this.selectedReason = target.value;
  }

  private closePopup(): void {
    this.close.emit();
  }

}
