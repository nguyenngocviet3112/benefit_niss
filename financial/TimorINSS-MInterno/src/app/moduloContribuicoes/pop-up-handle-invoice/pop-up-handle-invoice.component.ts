import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-pop-up-handle-invoice',
  templateUrl: './pop-up-handle-invoice.component.html',
})
export class PopUpHandleInvoiceComponent {
  invoice = {
    invNo: '90000320250301',
    submissionDate: '20/02/2025',
    paymentAmount: '221,20',
    actualReceived: '221,20',
    reason: ''
  };

  reasons = ['Duplicate', 'Incorrect amount', 'Missing data'];

  constructor(
    public dialogRef: MatDialogRef<PopUpHandleInvoiceComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  approve() {
    console.log('Approved with amount:', this.invoice.actualReceived);
    this.dialogRef.close({ status: 'approved', data: this.invoice });
  }

  reject() {
    console.log('Rejected with reason:', this.invoice.reason);
    this.dialogRef.close({ status: 'rejected', data: this.invoice });
  }
}
