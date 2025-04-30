import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowProductDetailComponent } from './show-product-detail.component';

describe('ShowProductDetailComponent', () => {
  let component: ShowProductDetailComponent;
  let fixture: ComponentFixture<ShowProductDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShowProductDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShowProductDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
