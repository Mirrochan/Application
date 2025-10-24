import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { EventsService } from '../../../data/services/events.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-delete-modal',
  imports: [CommonModule],
  templateUrl: './delete-modal.component.html',
  styleUrl: './delete-modal.component.scss'
})
export class DeleteModalComponent {
  @Input() isVisible: boolean = false;
  @Input() id: string = '';
  @Input() isDeleting: boolean = false;
  @Input() eventTitle: string = 'Are you sure you want to delete this item?';
  
  @Output() confirm = new EventEmitter<string>();
  @Output() cancel = new EventEmitter<void>();

  onConfirm(): void {
    this.confirm.emit(this.id); 
  }

  onCancel(): void {
    this.cancel.emit(); 
  }
 
}
