import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AnalisarComponent } from './analisar.component';

describe('AnalisarComponent', () => {
  let component: AnalisarComponent;
  let fixture: ComponentFixture<AnalisarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AnalisarComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AnalisarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
