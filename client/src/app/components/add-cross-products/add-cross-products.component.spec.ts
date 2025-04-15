import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddCrossProductsComponent } from './add-cross-products.component';

describe('AddCrossProductsComponent', () => {
  let component: AddCrossProductsComponent;
  let fixture: ComponentFixture<AddCrossProductsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddCrossProductsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddCrossProductsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
