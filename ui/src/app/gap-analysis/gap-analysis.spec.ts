import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { vi } from 'vitest';

import { GapAnalysisComponent } from './gap-analysis';

describe('GapAnalysisComponent', () => {
  let component: GapAnalysisComponent;
  let fixture: ComponentFixture<GapAnalysisComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GapAnalysisComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(GapAnalysisComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('calculateEmployeeScore', () => {
    it('should use AI score if present in results', () => {
      component.gapAnalysisResult = {
        summary: 'All good',
        gaps: [],
        scores: [
          { EmployeeEmail: 'test@example.com', Score: 85 }
        ]
      };
      
      const score = component.calculateEmployeeScore('test@example.com');
      expect(score).toBe(85);
    });

    it('should use AI score with lowercase properties if present in results', () => {
      component.gapAnalysisResult = {
        summary: 'All good',
        gaps: [],
        scores: [
          { employeeEmail: 'test@example.com', score: 92 }
        ]
      };
      
      const score = component.calculateEmployeeScore('test@example.com');
      expect(score).toBe(92);
    });

    it('should calculate fallback score based on gaps when AI score is not present', () => {
      component.requirements = [
        { skillId: 1, skillName: 'React', requiredLevel: 'Advanced' },
        { skillId: 2, skillName: 'Dotnet', requiredLevel: 'Intermediate' }
      ];

      component.gapAnalysisResult = {
        summary: 'Partial match',
        gaps: [
          { EmployeeEmail: 'test@example.com', SkillName: 'React', GapSeverity: 'No Gap' },
          { EmployeeEmail: 'test@example.com', SkillName: 'Dotnet', GapSeverity: 'Minor Gap' }
        ]
      };

      // React: No Gap = 100, Dotnet: Minor Gap = 75. Average = 87.5 => 88
      const score = component.calculateEmployeeScore('test@example.com');
      expect(score).toBe(88);
    });

    it('should calculate fallback score treating missing skill records as Critical Gap (0 points)', () => {
      component.requirements = [
        { skillId: 1, skillName: 'React', requiredLevel: 'Advanced' },
        { skillId: 2, skillName: 'Dotnet', requiredLevel: 'Intermediate' }
      ];

      component.gapAnalysisResult = {
        summary: 'Partial match',
        gaps: [
          { EmployeeEmail: 'test@example.com', SkillName: 'React', GapSeverity: 'Moderate Gap' }
          // Dotnet missing -> 0 points
        ]
      };

      // React: Moderate Gap = 50, Dotnet: missing = 0. Average = 25
      const score = component.calculateEmployeeScore('test@example.com');
      expect(score).toBe(25);
    });
  });

  describe('escapeCsvValue', () => {
    it('should return empty string for null or undefined', () => {
      expect(component.escapeCsvValue(null)).toBe('');
      expect(component.escapeCsvValue(undefined)).toBe('');
    });

    it('should leave normal string untouched', () => {
      expect(component.escapeCsvValue('hello')).toBe('hello');
    });

    it('should wrap string in quotes if it contains comma', () => {
      expect(component.escapeCsvValue('hello, world')).toBe('"hello, world"');
    });

    it('should escape double quotes and wrap in quotes if it contains quotes', () => {
      expect(component.escapeCsvValue('he said "yes"')).toBe('"he said ""yes"""');
    });
  });

  describe('modal and helper methods', () => {
    it('should open preview modal on exportReport', () => {
      component.gapAnalysisResult = { summary: 'Yes', gaps: [] };
      component.showPreviewModal = false;
      component.exportReport();
      expect(component.showPreviewModal).toBe(true);
      expect(component.reportDate).toBeInstanceOf(Date);
    });

    it('should close preview modal on closePreview', () => {
      component.showPreviewModal = true;
      component.closePreview();
      expect(component.showPreviewModal).toBe(false);
    });

    it('should return correct severity from getSkillGapLevel', () => {
      const gaps = [
        { SkillName: 'React', GapSeverity: 'Minor Gap' },
        { skillName: 'Dotnet', gapSeverity: 'Moderate Gap' }
      ];
      expect(component.getSkillGapLevel(gaps, 'React')).toBe('Minor Gap');
      expect(component.getSkillGapLevel(gaps, 'Dotnet')).toBe('Moderate Gap');
      expect(component.getSkillGapLevel(gaps, 'Angular')).toBe('Critical Gap');
    });

    it('should trigger print on printPdf', () => {
      const printSpy = vi.spyOn(window, 'print').mockImplementation(() => {});
      component.printPdf();
      expect(printSpy).toHaveBeenCalled();
      printSpy.mockRestore();
    });
  });
});
