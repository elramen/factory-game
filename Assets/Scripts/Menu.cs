public class Menu
{
    bool trashMode;
    int machineSelected;

    public Menu() {
        trashMode = false;
        machineSelected = 0;
    }

    public void toggleTrash() {
        machineSelected = 0;
        trashMode = !trashMode; 
    }

    public bool getTrashMode() {
        return trashMode;
    }

    public void toggleMachineSelected(int machine) {
        trashMode = false;

        if (machine == machineSelected) {
            machineSelected = 0;
        } else {
            machineSelected = machine;
        }
    }

    public int getMachineSelected() {
        return machineSelected;
    }

    public void HandleTouchStart() {
        
    }
}
