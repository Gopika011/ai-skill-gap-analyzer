export interface Project {
  id: number;
  name: string;
  description: string;
  startDate: string;
  endDate: string;
}

export interface ProjectEmployee {
  id: string;
  name: string;
  email: string;
}

export interface ProjectRequirement {
  skillId: number;
  skillName: string;
  requiredLevel: string;
}