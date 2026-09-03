export interface Project {
  id: string;
  name: string;
  description: string | null;
  ownerId: string;
  ownerName: string;
  createdAt: string;
  updatedAt: string | null;
}
