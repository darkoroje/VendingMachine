The project has no UI or command line interface, but test can be run to exercise functionality.  
It is assumed that the main program would instantiate the vending machine and other supporting classes, and would have a UI of some kind.


VendingMachine class has three methods that cover the specified features:

`AddCoin` should be called when the customer inserts a coin  
`CheckDisplay` to update the display  
`VendProduct` to vend the a product with the specified name. It will return correct change to the customer.  
<br />
The `IHardwareInterface` is the interface to the actual hardware, not implemented in this project but assumed to have basic functionallity to insert coins etc.
