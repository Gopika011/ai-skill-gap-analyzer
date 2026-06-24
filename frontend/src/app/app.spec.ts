import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { App } from './app';
import { AuthService } from './services/auth';
import { BehaviorSubject } from 'rxjs';

describe('App', () => {
  let currentUserSubject: BehaviorSubject<any>;
  let authServiceMock: any;

  beforeEach(async () => {
    currentUserSubject = new BehaviorSubject<any>(null);
    authServiceMock = {
      currentUser$: currentUserSubject.asObservable(),
      logout: () => {},
    };

    await TestBed.configureTestingModule({
      imports: [App],
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: authServiceMock }
      ],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render guest navigation links when logged out', async () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.app-nav')?.textContent).toContain('Login');
    expect(compiled.querySelector('.app-nav')?.textContent).not.toContain('Categories');
  });

  it('should render member navigation links when logged in', async () => {
    currentUserSubject.next({ id: 1, email: 'test@example.com' });
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.app-nav')?.textContent).toContain('Categories');
    expect(compiled.querySelector('.app-nav')?.textContent).toContain('test@example.com');
  });
});
