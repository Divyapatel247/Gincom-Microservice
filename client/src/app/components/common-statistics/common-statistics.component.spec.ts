import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommonStatisticsComponent } from './common-statistics.component';

describe('CommonStatisticsComponent', () => {
  let component: CommonStatisticsComponent;
  let fixture: ComponentFixture<CommonStatisticsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CommonStatisticsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CommonStatisticsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
