import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { EventsService } from '../../../data/services/events.service';
import { MyEvents } from '../../../data/interfaces/event.model';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-my-events',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './my-events.component.html',
  styleUrls: ['./my-events.component.scss']
})
export class MyEventsComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  events: MyEvents[] = [];
  viewMode: 'month' | 'week' = 'month';

  isEvents = true;
  currentDate: Date = new Date();
  currentMonthLabel: string = '';
  daysInMonth: number[] = [];
  startDayOffset: number = 0;
  currentWeekDays: Date[] = [];

  constructor(
    private eventService: EventsService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadEvents();
    this.updateCalendar();
  }

  loadEvents() {
    this.eventService.getUserEvents()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
      next: (data) => {
        (this.events = data)
        if (this.events.length == 0 || this.events ==null) { this.isEvents = false; } else { this.isEvents = true; }
      },
      error: (err) => console.error('Error loading events', err),
    });

  }

  updateCalendar() {
    const year = this.currentDate.getFullYear();
    const month = this.currentDate.getMonth();

    this.currentMonthLabel = this.currentDate.toLocaleString('default', {
      month: 'long',
      year: 'numeric'
    });

    const totalDays = new Date(year, month + 1, 0).getDate();
    this.daysInMonth = Array.from({ length: totalDays }, (_, i) => i + 1);
    this.startDayOffset = new Date(year, month, 1).getDay();

    const startOfWeek = this.getStartOfWeek(this.currentDate);
    this.currentWeekDays = Array.from({ length: 7 }, (_, i) => {
      const d = new Date(startOfWeek);
      d.setDate(startOfWeek.getDate() + i);
      return d;
    });
  }

  prevPeriod() {
    if (this.viewMode === 'month') {
      this.currentDate.setMonth(this.currentDate.getMonth() - 1);
    } else {
      this.currentDate.setDate(this.currentDate.getDate() - 7);
    }
    this.updateCalendar();
  }

  nextPeriod() {
    if (this.viewMode === 'month') {
      this.currentDate.setMonth(this.currentDate.getMonth() + 1);
    } else {
      this.currentDate.setDate(this.currentDate.getDate() + 7);
    }
    this.updateCalendar();
  }

  switchView(mode: 'month' | 'week') {
    this.viewMode = mode;
    this.updateCalendar();
  }

  getEventsForDay(day: number): MyEvents[] {
    const date = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth(), day);
    return this.events.filter(e =>
      new Date(e.date).toDateString() === date.toDateString()
    );
  }

  getEventsForWeekday(date: Date): MyEvents[] {
    return this.events.filter(e =>
      new Date(e.date).toDateString() === date.toDateString()
    );
  }

  openEvent(id: string) {
    this.router.navigate(['/event-details', id]);
  }


  isToday(day: number): boolean {
    const today = new Date();
    const cellDate = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth(), day);
    return today.toDateString() === cellDate.toDateString();
  }


  isTodayInWeek(date: Date): boolean {
    const today = new Date();
    return today.toDateString() === date.toDateString();
  }

  private getStartOfWeek(date: Date): Date {
    const start = new Date(date);
    const day = start.getDay();
    const diff = start.getDate() - day + (day === 0 ? -7 : 0);
    start.setDate(diff);
    return start;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}