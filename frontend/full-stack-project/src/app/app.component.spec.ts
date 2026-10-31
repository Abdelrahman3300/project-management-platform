import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { AppComponent } from './app.component';
import { ProjectService } from './services/project.service';
import { AuthService } from './services/auth.service';

describe('AppComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppComponent],
      providers:  [
        provideRouter([]),
        provideHttpClient(),
        {
          provide: ProjectService,
          useValue: {
            getAll: () => ({
              subscribe: () => {}
            })
          }
        },
        {
          provide: AuthService,
          useValue: {
            isLoggedIn: () => false,
            logout: () => {}
          }
        }
      ]
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    expect(fixture.componentInstance).toBeTruthy();
  });

});
