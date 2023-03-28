export interface Gift {
    id:          string;
    createdDate: Date;
    name:        string;
    description: string;
    price:       number;
    productUrl:  string;
    recipients:  any[];
  }