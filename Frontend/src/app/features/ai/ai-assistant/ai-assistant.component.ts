import { Component, OnInit, AfterViewChecked, ElementRef, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { aiAsistantConversation, aiAsistantResponce } from '../../../data/interfaces/ai.model';
import { AiAssistantService } from '../../../data/services/ai-assistant.service';
import { firstValueFrom } from 'rxjs';

type ChatMessage = aiAsistantResponce | LoadingMessage;

interface LoadingMessage {
  type: 'loading';
  id: string;
  question: string;
}

@Component({
  selector: 'app-ai-assistant',
  templateUrl: './ai-assistant.component.html',
  styleUrls: ['./ai-assistant.component.scss'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule]
})
export class AiAssistantComponent implements OnInit, AfterViewChecked {
  @ViewChild('chatContainer', { static: false }) private chatContainer!: ElementRef;
  @ViewChild('messageInput', { static: false }) private messageInput!: ElementRef;

  chatForm: FormGroup;
  messages: ChatMessage[] = [];
  isSending = false;
  conversationHistory: aiAsistantConversation[] = [];
  private shouldScroll = false;

  quickQuestions = [
    "What events am I attending this week?",
    "When is my next event?",
    "List all events I organize",
    "Show public tech events this weekend.",
    "Who’s attending the Marketing Meetup?",
    "Where is the Design Sprint?"
  ];

  constructor(
    private fb: FormBuilder,
    private aiAssistantService: AiAssistantService
  ) {
    this.chatForm = this.fb.group({
      question: ['', [Validators.required, Validators.minLength(1)]]
    });
  }

  ngOnInit() {
    this.loadConversationHistory();
  }

  ngAfterViewChecked() {
    if (this.shouldScroll) {
      this.chatContainer.nativeElement.scrollTop = this.chatContainer.nativeElement.scrollHeight;
      this.shouldScroll = false;
    }
  }

  loadConversationHistory() {
    this.conversationHistory = [];
  }

  async sendMessage() {
    if (this.chatForm.invalid || this.isSending) return;

    const question = this.chatForm.get('question')?.value.trim();
    if (!question) return;

    this.isSending = true;

    const userMessage: aiAsistantResponce = {
      id: crypto.randomUUID(),
      question,
      answer: '',
      askedAt: new Date()
    };
    this.messages.push(userMessage);

    const loadingMessage: LoadingMessage = {
      type: 'loading',
      id: crypto.randomUUID(),
      question
    };
    this.messages.push(loadingMessage);

    this.chatForm.reset();
    this.scrollToBottom();

    this.aiAssistantService.askQuestion({ question }).subscribe({
      next: (response) => {
        this.messages = this.messages.filter(msg => !this.isLoadingMessage(msg) || msg.id !== loadingMessage.id);

        if (response) {
          userMessage.answer = response.answer;
          userMessage.askedAt = response.askedAt || new Date();

          this.conversationHistory.push({
            answer: response.answer,
            askedAt: userMessage.askedAt
          });
        } else {
          userMessage.answer = 'Sorry, no response received from the AI assistant. Please try again.';
        }
      },
      error: (error) => {
        console.error('Error sending message:', error);
        this.messages = this.messages.filter(msg => !this.isLoadingMessage(msg) || msg.id !== loadingMessage.id);
        userMessage.answer = 'Sorry, I encountered an error. Please try again.';
      },
      complete: () => {
        this.isSending = false;
        this.scrollToBottom();
        this.focusInput();
      }
    });

  }

  useQuickQuestion(question: string) {
    this.chatForm.patchValue({ question });
    this.sendMessage();
  }

  onKeyPress(event: KeyboardEvent) {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    }
  }

  isUserMessage(message: ChatMessage): message is aiAsistantResponce {
    return !this.isLoadingMessage(message);
  }

  isLoadingMessage(message: ChatMessage): message is LoadingMessage {
    return (message as LoadingMessage).type === 'loading';
  }

  hasAnswer(message: ChatMessage): message is aiAsistantResponce {
    return this.isUserMessage(message) && !!message.answer;
  }

  formatTime(date?: Date): string {
    return date ? new Date(date).toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' }) : '';
  }

  formatDate(date?: Date): string {
    return date ? new Date(date).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' }) : '';
  }


  private scrollToBottom(): void {
    this.shouldScroll = true;
  }

  private focusInput(): void {
    this.messageInput?.nativeElement.focus();
  }

  autoResize(event: Event) {
    const target = event.target as HTMLTextAreaElement;
    target.style.height = 'auto';
    target.style.height = target.scrollHeight + 'px';
  }
}
