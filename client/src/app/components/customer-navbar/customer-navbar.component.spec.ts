import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerNavbarComponent } from './customer-navbar.component';

describe('CustomerNavbarComponent', () => {
  let component: CustomerNavbarComponent;
  let fixture: ComponentFixture<CustomerNavbarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CustomerNavbarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CustomerNavbarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
