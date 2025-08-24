export interface Employee {
  employeeId: number;
  employeeName: string;
  age: number;
  mobileNum: string;
  dob?: string;
  addressLine1: string;
  addressLine2?: string;
  pincode: string;
  countryId: number;
  countryName?: string;
  stateId: number;
  stateName?: string;
}
