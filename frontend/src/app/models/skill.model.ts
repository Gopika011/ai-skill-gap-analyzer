export interface Category {
  id: number;
  categoryName: string;
  description?: string;
}

export interface Skill {
  id: number;
  skillName: string;
  description?: string;
  categoryId: number;
  categoryName?: string;
}

export interface EmployeeSkill {
  id: number;
  userId: number;
  skillId: number;
  skillName: string;
  categoryName: string;
  proficiencyLevel: string;
  yearsOfExperience: number;
}