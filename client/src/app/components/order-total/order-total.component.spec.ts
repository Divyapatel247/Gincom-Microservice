import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderTotalComponent } from './order-total.component';

describe('OrderTotalComponent', () => {
  let component: OrderTotalComponent;
  let fixture: ComponentFixture<OrderTotalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderTotalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrderTotalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
