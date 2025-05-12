import { Component, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-pop-up-handle-invoice',
  templateUrl: './pop-up-handle-invoice.component.html',
  styleUrls: ['./pop-up-handle-invoice.component.css']
})
export class PopUpHandleInvoiceComponent {
  invoice = {
    invNo: '900003205030301',
    submissionDate: '20/02/2025',
    paymentAmount: '$221.20',
    actualReceive: '$221.20'
  };

  reasons = ['Choose a reason', 'Invalid Receipt', 'Amount Mismatch', 'Duplicate Submission'];
  selectedReason: string = this.reasons[0];
  statusMessage: string = '';

  @Output() approve = new EventEmitter<void>();
  @Output() reject = new EventEmitter<string>();
  @Output() cancel = new EventEmitter<void>();
  @Output() close = new EventEmitter<void>();

  onApprove(): void {
    this.statusMessage = 'Payment approved successfully!';
    this.approve.emit();
    setTimeout(() => this.closePopup(), 1500); // Close after 1.5 seconds
  }

  onReject(): void {
    if (this.selectedReason === 'Choose a reason') {
      this.statusMessage = 'Please select a reason for rejection.';
      return;
    }
    this.statusMessage = `Payment rejected: ${this.selectedReason}`;
    this.reject.emit(this.selectedReason);
    setTimeout(() => this.closePopup(), 1500);
  }

  onCancel(): void {
    this.statusMessage = 'Action canceled.';
    this.cancel.emit();
    setTimeout(() => this.closePopup(), 1500);
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
